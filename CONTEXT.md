# Xoderony Stride UI

This context defines the generated, strongly typed views through which game code accesses Stride UI assets and their live instances.

## Language

**Page View**:
A generated component representing the live instance of a UI page displayed by the `UIComponent` on the same Entity.
_Avoid_: Page binding, UI binding

**Library**:
A generated serializable value representing a UI library and providing strongly typed creation of its root elements.
_Avoid_: Library View, library component, library binding

**Library Instance View**:
A generated object representing one instantiated root subtree created through a Library.
_Avoid_: Library component, template binding
