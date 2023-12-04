resource "azurerm_storage_account" "backend" {
  name                     = "${replace(var.app_project_slug, "-", "")}${replace(var.app_environment, "-", "")}backend"
  resource_group_name      = var.app_project_slug
  location                 = var.arm_location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}
