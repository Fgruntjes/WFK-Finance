resource "azurerm_servicebus_namespace" "service_bus" {
  name                = "${var.app_project_slug}-${var.app_environment}"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
  sku                 = "Basic"
  local_auth_enabled  = true

  network_rule_set {
    trusted_services_allowed      = true
    public_network_access_enabled = local.environment_data_ephemeral
    ip_rules                      = local.environment_data_ephemeral ? ["0.0.0.0/0"] : []
  }

  tags = {
    environment = var.app_environment
  }
}
