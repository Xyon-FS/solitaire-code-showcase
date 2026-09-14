using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire
{
    public sealed class SolitaireBoardController : MonoBehaviour
    {
        [SerializeField] private DeckDefinition deckDefinition;
        [SerializeField] private CardView cardPrefab;
        [SerializeField] private List<CardPileView> tableauPiles = new List<CardPileView>();
        [SerializeField] private bool shuffleDeck = true;

        private readonly Dictionary<Card, CardView> cardViews =
            new Dictionary<Card, CardView>();

        private CommandExecutor commandExecutor;
        private CommandHistory commandHistory;
        private MoveController moveController;
        private bool isInitialized;
        
        [SerializeField] private CardPileView deckPileView;
        

        public CommandHistory CommandHistory
        {
            get
            {
                return commandHistory;
            }
        }

        public MoveController MoveController
        {
            get
            {
                return moveController;
            }
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            ValidateConfiguration();

            commandExecutor = new CommandExecutor();
            commandHistory = new CommandHistory();
            moveController = new MoveController(commandExecutor, commandHistory);
            
            
            var deck = new Deck(deckDefinition);

            if (shuffleDeck)
            {
                deck.Shuffle();
            }

            InitializePiles();
            InitializeDeck();
            DealTableau(deck);
            FillDeck(deck);
            RevealInitialDeckCard();
            RefreshViews();
            isInitialized = true;
        }

        public void RefreshViews()
        {
            for (int i = 0; i < tableauPiles.Count; i++)
            {
                RefreshPileView(tableauPiles[i]);
            }

            RefreshPileView(deckPileView);
        }

        private void RefreshPileView(CardPileView pileView)
        {
            for (int cardIndex = 0; cardIndex < pileView.Pile.Cards.Count; cardIndex++)
            {
                Card card = pileView.Pile.Cards[cardIndex];
                CardView cardView = cardViews[card];

                pileView.Place(cardView, cardIndex);
            }
        }

        private void InitializePiles()
        {
            for (int i = 0; i < tableauPiles.Count; i++)
            {
                CardPileView pileView = tableauPiles[i];
                string pileId = string.IsNullOrWhiteSpace(pileView.PileId)
                    ? $"tableau_{i + 1}"
                    : pileView.PileId;

                pileView.Initialize(new CardPile(pileId));
            }
        }
        
        public void UndoLastMove()
        {
            IReadOnlyList<IUndoableCommand> operation =
                commandHistory.UndoLast();

            if (operation == null)
            {
                return;
            }

            for (int i = 0; i < operation.Count; i++)
            {
                MoveCardCommand moveCommand =
                    operation[i] as MoveCardCommand;

                if (moveCommand != null)
                {
                    RestoreCardView(moveCommand);
                    continue;
                }

                RevealTopCardCommand revealCommand =
                    operation[i] as RevealTopCardCommand;

                if (revealCommand != null &&
                    revealCommand.RevealedCard != null)
                {
                    CardView revealedCardView =
                        cardViews[revealCommand.RevealedCard];

                    revealedCardView.RefreshFacing();
                }
            }
        }
        
        private void RestoreCardView(MoveCardCommand command)
        {
            CardView cardView = cardViews[command.Card];
            CardPileView sourceView = FindPileView(command.Source);

            sourceView.Place(cardView, command.SourceIndex);
        }
        
        private void RefreshTopCardFacing(CardPileView pileView)
        {
            Card topCard = pileView.Pile.TopCard;

            if (topCard == null)
            {
                return;
            }

            CardView topCardView = cardViews[topCard];
            topCardView.RefreshFacing();
        }
        
        private CardPileView FindPileView(CardPile pile)
        {
            if (ReferenceEquals(deckPileView.Pile, pile))
            {
                return deckPileView;
            }

            for (int i = 0; i < tableauPiles.Count; i++)
            {
                CardPileView pileView = tableauPiles[i];

                if (ReferenceEquals(pileView.Pile, pile))
                {
                    return pileView;
                }
            }

            throw new InvalidOperationException(
                $"No view found for pile '{pile.Id}'.");
        }

        private void DealTableau(Deck deck)
        {
            for (int pileIndex = 0; pileIndex < tableauPiles.Count; pileIndex++)
            {
                CardPile pile = tableauPiles[pileIndex].Pile;

                for (int cardIndex = 0; cardIndex <= pileIndex; cardIndex++)
                {
                    if (!deck.TryDraw(out Card card))
                    {
                        throw new InvalidOperationException("The deck does not contain enough cards to deal the tableau.");
                    }

                    card.SetFaceUp(cardIndex == pileIndex);
                    pile.Add(card);
                    CreateCardView(card);
                }
            }
        }

        private void CreateCardView(Card card)
        {
            CardView cardView = Instantiate(cardPrefab, transform);
            cardView.Initialize(card);
            cardViews.Add(card, cardView);
        }
        
        private void InitializeDeck()
        {
            string deckId = deckPileView.PileId;
            deckPileView.Initialize(new CardPile(deckId));
        }
        
        private void FillDeck(Deck deck)
        {
            while (deck.TryDraw(out Card card))
            {
                card.SetFaceUp(false);
                deckPileView.Pile.Add(card);
                CreateCardView(card);
            }
        }
        
        private void RevealInitialDeckCard()
        {
            RevealTopCardCommand revealCommand =
                new RevealTopCardCommand(deckPileView.Pile);

            commandExecutor.Execute(revealCommand);
        }
        
        public bool CanDrag(CardView cardView)
        {
            if (cardView == null || cardView.Card == null || !cardView.Card.IsFaceUp)
            {
                return false;
            }

            CardPileView owner = cardView.Owner;

            if (owner == null || owner.Pile == null || owner.Pile.Count == 0)
            {
                return false;
            }

            Card topCard = owner.Pile.Cards[owner.Pile.Count - 1];
            return ReferenceEquals(topCard, cardView.Card);
        }

        public bool TryMove(CardView cardView, CardPileView destination)
        {
            if (cardView == null || destination == null)
            {
                return false;
            }

            CardPileView source = cardView.Owner;

            if (source == null || source.Pile == null || destination.Pile == null)
            {
                return false;
            }

            if (!moveController.Move(cardView.Card, source.Pile, destination.Pile))
            {
                return false;
            }

            int destinationIndex = destination.Pile.Count - 1;
            destination.Place(cardView, destinationIndex);
            
            RefreshTopCardFacing(source);
            return true;
        }

        private void ValidateConfiguration()
        {
            if (deckDefinition == null)
            {
                throw new InvalidOperationException("The deck definition is not assigned.");
            }

            if (cardPrefab == null)
            {
                throw new InvalidOperationException("The card prefab is not assigned.");
            }

            if (tableauPiles.Count == 0)
            {
                throw new InvalidOperationException("At least one tableau pile view is required.");
            }
            
            if (deckPileView == null)
            {
                throw new InvalidOperationException("The deck pile view is not assigned.");
            }

            for (int i = 0; i < tableauPiles.Count; i++)
            {
                if (tableauPiles[i] == null)
                {
                    throw new InvalidOperationException($"The tableau pile view at index {i} is not assigned.");
                }
            }
            
        }
    }
}
