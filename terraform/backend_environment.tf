resource "azurerm_log_analytics_workspace" "backend" {
  name                = "v${var.app_environment}-backend"
  location            = var.arm_location
  resource_group_name = var.app_project_slug
  sku                 = "PerGB2018"
}

resource "azurerm_container_app_environment" "backend" {
  name                       = "v${var.app_environment}-backend"
  location                   = var.arm_location
  resource_group_name        = var.app_project_slug
  log_analytics_workspace_id = azurerm_log_analytics_workspace.backend.id

  tags = {
    environment = var.app_environment
  }
}

#resource "azurerm_service_plan" "backend" {
#  name                       = "v${var.app_environment}-backend"
#  resource_group_name        = var.app_project_slug
#  location                   = var.arm_location
#  app_service_environment_id = azurerm_container_app_environment.backend.id
#  os_type                    = "Linux"
#  sku_name                   = "Y1"
#}
