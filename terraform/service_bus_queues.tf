locals {
  service_bus_queues = {
    # <queue_name>: <worker_project> (@see App.Lib.ServiceBus.GetQueueName)
    Institution-TransactionImport : "App.Institution.Job.TransactionImport"
  }
}

# Error / dead letter queue
resource "azurerm_servicebus_queue" "queue_error" {
  name         = "error"
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_servicebus_queue" "queues" {
  for_each     = local.service_bus_queues
  name         = each.key
  namespace_id = azurerm_servicebus_namespace.service_bus.id
}

resource "azurerm_user_assigned_identity" "service_bus_receive" {
  for_each            = local.service_bus_queues
  name                = "${var.app_environment}-service_bus-receive-${each.key}"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_role_assignment" "service_bus_receive_queue" {
  for_each             = local.service_bus_queues
  scope                = azurerm_servicebus_queue.queues[each.key].id
  role_definition_name = "Azure Service Bus Data Receiver"
  principal_id         = azurerm_user_assigned_identity.service_bus_receive[each.key].principal_id
}
