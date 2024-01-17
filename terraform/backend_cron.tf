locals {
  cronjobs = {
    "institutionaccountimport" = {
      endpoint  = "/institutionaccounts/all/cron/import"
      frequency = "Hour"
      interval  = 12
    },
  }
}
resource "azurerm_logic_app_workflow" "cron" {
  for_each            = local.cronjobs
  name                = "${var.app_environment}-cron-trigger-${each.key}"
  location            = var.arm_location
  resource_group_name = var.app_project_slug

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_logic_app_trigger_recurrence" "cron" {
  for_each     = local.cronjobs
  name         = "schedule"
  logic_app_id = azurerm_logic_app_workflow.cron[each.key].id
  frequency    = each.value.frequency
  interval     = each.value.interval
}

resource "azurerm_logic_app_action_http" "cron" {
  for_each     = local.cronjobs
  name         = "urlcall"
  logic_app_id = azurerm_logic_app_workflow.cron[each.key].id
  method       = "POST"
  uri          = "${local.app_api_url}${each.value.endpoint}?cronToken=${local.backend_settings.App.CronToken}"
}
