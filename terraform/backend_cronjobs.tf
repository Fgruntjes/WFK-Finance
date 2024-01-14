locals {
  cronjobs = {
    institution-transactionimport = "App.Institution.Cron.TransactionImport"
  }
}

resource "azurerm_storage_account" "backend_cronjobs" {
  for_each                 = local.cronjobs
  name                     = substr(sha1("${var.app_project_slug}${var.app_environment}${each.key}"), 0, 24)
  resource_group_name      = var.app_project_slug
  location                 = var.arm_location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags = {
    environment = var.app_environment
    cronjob     = each.value
  }
}

resource "azurerm_user_assigned_identity" "backend_cronjobs_storage" {
  for_each            = local.cronjobs
  name                = "${var.app_environment}-cron-storage-${each.key}"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_role_assignment" "backend_cronjobs_storage" {
  for_each             = local.cronjobs
  scope                = azurerm_storage_account.backend_cronjobs[each.key].id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_user_assigned_identity.backend_cronjobs_storage[each.key].principal_id
}

resource "azurerm_linux_function_app" "backend_cronjobs" {
  for_each                      = local.cronjobs
  name                          = "v${var.app_environment}-cron-${each.key}"
  location                      = var.arm_location
  resource_group_name           = var.app_project_slug
  storage_account_name          = azurerm_storage_account.backend_cronjobs[each.key].name
  storage_uses_managed_identity = true
  service_plan_id               = azurerm_service_plan.backend.id

  site_config {
    container_registry_use_managed_identity = true
    app_scale_limit                         = 1
    application_stack {
      docker {
        image_name   = "${data.azurerm_container_registry.app.login_server}/${lower(each.value)}:${var.app_version}"
        image_tag    = var.app_version
        registry_url = data.azurerm_container_registry.app.login_server
      }
    }
  }

  tags = {
    environment = var.app_environment
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.backend_database_read_write.id,
      azurerm_user_assigned_identity.keyvault.id,
      azurerm_user_assigned_identity.container_registry_pull.id,
      azurerm_user_assigned_identity.service_bus_send.id,
      azurerm_user_assigned_identity.backend_cronjobs_storage[each.key].id
    ]
  }

  depends_on = [azurerm_role_assignment.container_registry_pull]
}
