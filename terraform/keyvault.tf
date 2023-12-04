resource "azurerm_key_vault" "app" {
  name                       = "${var.app_project_slug}-${var.app_environment}"
  location                   = var.arm_location
  resource_group_name        = var.app_project_slug
  tenant_id                  = var.arm_tenant_id
  soft_delete_retention_days = 7
  purge_protection_enabled   = false

  sku_name = "standard"
}

resource "azurerm_key_vault_access_policy" "cicd" {
  key_vault_id = azurerm_key_vault.app.id
  tenant_id    = var.arm_tenant_id
  object_id    = data.azuread_client_config.current.object_id

  key_permissions = [
    "Get",
    "GetRotationPolicy",
    "Create",
    "Delete",
    "Purge",
    "Update",
    "Rotate",
  ]
}
