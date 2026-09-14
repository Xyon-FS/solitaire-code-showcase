# Solitaire Developer Case Study

## What I built

A small modular Solitaire prototype in Unity. It includes a configurable deck definition, 
runtime card and pile models, shuffled tableau setup, drag-and-drop movement,
automatic reveal of newly exposed cards, undo support implemented with the Command pattern and textured 3D card views.
The model, presentation, input, commands, and history are kept as separate responsibilities.


## What I would improve with more time

I would add the complete Solitaire rule set and win conditions, a conventional stock/waste flow, 
move and flip animations, stronger interaction-state handling, and broader automated test coverage.

I would replace runtime `GetComponent` and `GetComponentInParent` lookups with cached or explicitly
assigned component references where appropriate. These lookups currently occur only on pointer press
and release, so their performance cost is negligible, but removing them would make dependencies more
explicit and slightly reduce runtime overhead.

I would also replace the Undo button's direct reference to `SolitaireBoardController` with a
ScriptableObject-based event channel. The UI would raise an undo request through the shared asset
and the controller would subscribe to it, avoiding fragile scene references in the Inspector and 
allowing both components to remain independently reusable.

Finally, I would profile initialization and introduce CardView pooling only if repeated game 
resets or measured performance justified it.

## AI assistance

AI was used as a pair-programming and review tool. I prompted it to propose a simple, expandable architecture;
scaffold parts of the model, command/history, presentation, drag, and test code; and review specific design
and debugging questions such as incremental view updates, undoing grouped operations, and avoiding unnecessary
coupling or allocations. I iteratively challenged and refined its suggestions rather than accepting them unchanged.
Final architectural choices, Unity scene and prefab setup, card materials and assets, Inspector configuration,
and runtime validation were handled manually.

## Project structure

- `Assets/Solitaire/Runtime/Models`: runtime card, deck, and pile data, independent from the presentation layer.
- `Assets/Solitaire/Runtime/Application/Commands`: command contracts, execution, and reversible move/reveal operations.
- `Assets/Solitaire/Runtime/Application/Controllers`: coordination of moves and command history.
- `Assets/Solitaire/Runtime/Presentation/Views`: MonoBehaviours responsible for displaying cards, piles, and UI elements.
- `Assets/Solitaire/Runtime/Presentation/Controllers`: board initialization, model-view synchronization, input, and drag-and-drop interaction.
- `Assets/Solitaire/Runtime/Utils`: deck configuration and card-definition generation utilities.
- `Assets/Solitaire/Editor`: custom Inspector and property-drawer tooling for deck configuration.
- `Assets/Solitaire/Config`: configured Unity assets, including the standard deck definition.
- `Assets/Solitaire/Tests/EditMode`: automated tests for the core deck, generation, and movement logic.