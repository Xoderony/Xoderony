# Separate page view and library lifecycles

Stride UI Page instances belong to an Entity through `UIComponent`, while UI library roots are templates instantiated as independent UI subtrees. Generate Page Views as `StartupScript` components and Libraries as serializable values that create ordinary Library Instance Views; do not force them behind one component model, because doing so would obscure their different ownership and initialization boundaries.
