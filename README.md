# DH-UI

Task for [NAME DELETED] company.

## Implemented

— Party character switching with updates for portrait, name, HP/Armor, abilities list, and modifications list;

— Individual data sets per character (preserved when switching);

— Drag & Drop assignment of a modifier to an ability;

— Reassignment of an already assigned modifier from one ability to another;

— Type-based compatibility checks (`Psyker`, `Dot`, `Attack`, `Buff`, *`Debuff`);

— Compatible target highlighting on hover/drag (during drag: only valid abilities are highlighted);

— *Top tab panel control;

— Modifier removal from an ability (RightClick).

`*` — Optional requirements.

## Results

https://github.com/user-attachments/assets/87ec659a-51da-4ebd-bad8-f1f3732b38c2

## Technologies

— Unity (uGUI), C#;

— MVVM (Model / ViewModel / View);

— Command pattern for assign/remove operations;

— Basic custom `ObservableProperty<T>` for reactive UI updates.
