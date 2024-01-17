resource "azurerm_container_app" "service_bus_worker" {
  for_each                     = local.service_bus_queues
  name                         = substr("v${var.app_environment}-${lower(each.key)}", 0, 32)
  container_app_environment_id = azurerm_container_app_environment.backend.id
  resource_group_name          = var.app_project_slug
  revision_mode                = "Single"

  tags = {
    environment = var.app_environment
  }

  depends_on = [azurerm_role_assignment.container_registry_pull]

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.backend_database_read_write.id,
      azurerm_user_assigned_identity.container_registry_pull.id,
      azurerm_user_assigned_identity.keyvault.id,
      azurerm_user_assigned_identity.service_bus_send.id,
      azurerm_user_assigned_identity.service_bus_receive[each.key].id,
    ]
  }

  registry {
    server   = data.azurerm_container_registry.app.login_server
    identity = azurerm_user_assigned_identity.container_registry_pull.id
  }

  secret {
    name = "settings"
    value = jsonencode(merge(local.backend_settings, {
      ConnectionStrings = {
        Database   = local.connection_strings.database.readwrite
        ServiceBus = local.connection_strings.service_bus.prod
      }
      ServiceBus = {
        AzureClientId = azurerm_user_assigned_identity.service_bus_receive[each.key].client_id
      }
    }))
  }
  secret {
    name  = "scale-rule-auth"
    value = local.connection_strings.service_bus.autoscaler
  }

  template {
    min_replicas = 0
    max_replicas = 10

    custom_scale_rule {
      name             = "azure-servicebus"
      custom_rule_type = "azure-servicebus"
      metadata = {
        queueName              = lower(each.key)
        messageCount           = 20
        activationMessageCount = 1
      }
      authentication {
        secret_name       = "scale-rule-auth"
        trigger_parameter = "connection"
      }
    }

    container {
      name  = "app"
      image = "${data.azurerm_container_registry.app.login_server}/${lower(each.value)}:${var.app_version}"
      command = [
        "bash",
        "-c",
        "echo \"$${AppSettings}\" > appsettings.local.json && dotnet ${each.value}.dll",
      ]
      cpu    = 0.5
      memory = "1Gi"
      env {
        name        = "AppSettings"
        secret_name = "settings"
      }
      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }
    }
  }
}
