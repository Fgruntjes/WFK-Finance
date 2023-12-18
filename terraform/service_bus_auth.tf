resource "azurerm_user_assigned_identity" "service_bus_send" {
  name                = "${var.app_environment}-service_bus-send"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}
resource "azurerm_role_assignment" "service_bus_send" {
  scope                = azurerm_servicebus_namespace.service_bus.id
  role_definition_name = "Azure Service Bus Data Sender"
  principal_id         = azurerm_user_assigned_identity.service_bus_send.principal_id
}

resource "azurerm_servicebus_namespace_authorization_rule" "service_bus" {
  count        = local.environment_data_ephemeral ? 1 : 0
  name         = "${var.app_project_slug}-cicd"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
  listen       = true
  send         = true
  manage       = false
}

resource "azurerm_servicebus_namespace_authorization_rule" "autoscaler" {
  name         = "${var.app_project_slug}-autoscaler"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
  listen       = true
  send         = true
  manage       = true
}