resource "azurerm_user_assigned_identity" "container_registry_pull" {
  name                = "${var.app_environment}-container-registry-pull"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_role_assignment" "container_registry_pull" {
  scope                = data.azurerm_container_registry.app.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.container_registry_pull.principal_id
}
