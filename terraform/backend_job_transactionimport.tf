#locals {
#  cronjobs = {
#    institution-transactionimport = "App.Institution.Cron.TransactionImport"
#  }
#}
#resource "azurerm_linux_function_app" "cron_transactionimport" {
#  for_each            = local.cronjobs
#  name                = "v${var.app_environment}-cron-${each.key}"
#  location            = var.arm_location
#  resource_group_name = var.app_project_slug
#  service_plan_id     = azurerm_service_plan.backend.id
#
#  site_config {
#    container_registry_use_managed_identity = true
#    app_scale_limit                         = 1
#    linux_fx_version                        = "DOCKER"
#    application_stack {
#      docker {
#        image_name   = "${data.azurerm_container_registry.app.login_server}/${lower(each.value)}:${var.app_version}"
#        image_tag    = var.app_version
#        registry_url = data.azurerm_container_registry.app.login_server
#      }
#    }
#  }
#
#  tags = {
#    environment = var.app_environment
#  }
#
#  identity {
#    type = "UserAssigned"
#    identity_ids = [
#      azurerm_user_assigned_identity.backend_database_read_write.id,
#      azurerm_user_assigned_identity.keyvault.id,
#      azurerm_user_assigned_identity.container_registry_pull.id,
#      azurerm_user_assigned_identity.service_bus_send.id,
#    ]
#  }
#
#  depends_on = [azurerm_role_assignment.container_registry_pull]
#}
