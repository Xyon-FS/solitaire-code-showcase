using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Solitaire
{
    public sealed class CardDragController : MonoBehaviour
    {
        [SerializeField] private Camera inputCamera;
        [SerializeField] private SolitaireBoardController board;
        [SerializeField] private Transform dragRoot;
        [SerializeField] private LayerMask interactionLayers = ~0;
        [SerializeField] private float maxRayDistance = 100f;
        [SerializeField] private float dragLift = 0.1f;

        private CardView draggedCard;
        private CardPileView sourcePile;
        private Collider[] draggedColliders;
        private Plane dragPlane;
        private Vector3 dragNormal;
        private Vector3 pointerOffset;
        private int sourceIndex;

        private void Update()
        {
            Mouse mouse = Mouse.current;

            if (mouse == null)
            {
                return;
            }

            Vector2 pointerPosition = mouse.position.ReadValue();

            if (draggedCard == null && mouse.leftButton.wasPressedThisFrame)
            {
                TryBeginDrag(pointerPosition);
            }

            if (draggedCard == null)
            {
                return;
            }

            if (mouse.leftButton.isPressed)
            {
                UpdateDrag(pointerPosition);
            }

            if (mouse.leftButton.wasReleasedThisFrame)
            {
                EndDrag(pointerPosition);
            }
        }

        private void TryBeginDrag(Vector2 pointerPosition)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            CardView cardView = FindCard(pointerPosition);

            if (cardView == null || !board.CanDrag(cardView))
            {
                return;
            }

            sourcePile = cardView.Owner;
            sourceIndex = sourcePile.Pile.IndexOf(cardView.Card);
            dragNormal = GetNormalFacingCamera(sourcePile.transform.forward, cardView.transform.position);
            dragPlane = new Plane(dragNormal, cardView.transform.position);

            if (!TryGetPointOnDragPlane(pointerPosition, out Vector3 pointerWorldPosition))
            {
                sourcePile = null;
                return;
            }

            draggedCard = cardView;
            pointerOffset = draggedCard.transform.position - pointerWorldPosition;
            draggedCard.transform.SetParent(dragRoot, true);
            draggedColliders = draggedCard.GetComponentsInChildren<Collider>(true);
            SetDraggedCollidersEnabled(false);
            UpdateDrag(pointerPosition);
        }

        private void UpdateDrag(Vector2 pointerPosition)
        {
            if (!TryGetPointOnDragPlane(pointerPosition, out Vector3 pointerWorldPosition))
            {
                return;
            }

            draggedCard.transform.position =
                pointerWorldPosition + pointerOffset + dragNormal * dragLift;
        }

        private void EndDrag(Vector2 pointerPosition)
        {
            CardPileView destination = FindPile(pointerPosition);
            bool moved = destination != null && board.TryMove(draggedCard, destination);

            if (!moved)
            {
                sourcePile.Place(draggedCard, sourceIndex);
            }

            SetDraggedCollidersEnabled(true);
            ClearDrag();
        }

        private CardView FindCard(Vector2 pointerPosition) 
        {

            RaycastHit hit = GetHit(pointerPosition);
            if (hit.collider == null) return null;
            
            CardView cardView = hit.collider.GetComponent<CardView>();
            if (cardView != null)
            {
                return cardView;
            }
            return null;
        }

        private CardPileView FindPile(Vector2 pointerPosition) 
        {
            
            RaycastHit hit = GetHit(pointerPosition);

            if (hit.collider == null) return null;
            
            CardPileView pileView = hit.collider.GetComponentInParent<CardPileView>();
            if (pileView != null)
            {
                return pileView;
            }

            return null;
        }

        private RaycastHit GetHit(Vector2 pointerPosition)
        {
            Ray ray = inputCamera.ScreenPointToRay(pointerPosition);
            
            Physics.Raycast(ray, out RaycastHit hitInfo, maxRayDistance, interactionLayers,
                QueryTriggerInteraction.Ignore);
            
            return hitInfo;
        }

        private bool TryGetPointOnDragPlane(
            Vector2 pointerPosition,
            out Vector3 worldPosition)
        {
            Ray ray = inputCamera.ScreenPointToRay(pointerPosition);

            if (dragPlane.Raycast(ray, out float distance))
            {
                worldPosition = ray.GetPoint(distance);
                return true;
            }

            worldPosition = default;
            return false;
        }

        private Vector3 GetNormalFacingCamera(Vector3 normal, Vector3 cardPosition)
        {
            Vector3 directionToCamera = inputCamera.transform.position - cardPosition;

            if (Vector3.Dot(normal, directionToCamera) < 0f)
            {
                return -normal;
            }

            return normal;
        }

        private void SetDraggedCollidersEnabled(bool isEnabled)
        {
            if (draggedColliders == null)
            {
                return;
            }

            for (int i = 0; i < draggedColliders.Length; i++)
            {
                draggedColliders[i].enabled = isEnabled;
            }
        }

        private void ClearDrag()
        {
            draggedCard = null;
            sourcePile = null;
            draggedColliders = null;
            sourceIndex = -1;
        }

        private static int CompareHitDistance(RaycastHit left, RaycastHit right)
        {
            return left.distance.CompareTo(right.distance);
        }
    }
}
