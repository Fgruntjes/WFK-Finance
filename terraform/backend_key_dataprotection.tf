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
