
resource "azurerm_storage_container" "backend_data_protection" {
  name                  = "data-protection"
  storage_account_name  = azurerm_storage_account.backend.name
  container_access_type = "private"
}

resource "azurerm_storage_blob" "backend_data_protection" {
  name                   = "keys.xml"
  storage_account_name   = azurerm_storage_account.backend.name
  storage_container_name = azurerm_storage_container.backend_data_protection.name
  type                   = "Block"
}

resource "azurerm_key_vault_key" "backend_data_protection" {
  name         = "${var.app_environment}-backend-data-protection"
  key_vault_id = azurerm_key_vault.app.id
  key_type     = "RSA"
  key_size     = 2048

  key_opts = [
    "unwrapKey",
    "wrapKey",
  ]

  depends_on = [
    azurerm_key_vault_access_policy.cicd
  ]
}
