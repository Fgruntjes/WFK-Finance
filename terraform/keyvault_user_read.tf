resource "azurerm_user_assigned_identity" "keyvault" {
  name                = "${var.app_environment}-keyvault"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_key_vault_access_policy" "keyvault" {
  key_vault_id = azurerm_key_vault.app.id
  tenant_id    = var.arm_tenant_id
  object_id    = azurerm_user_assigned_identity.keyvault.client_id

  key_permissions = [
    "Get",
    "UnwrapKey",
    "WrapKey",
  ]
}
