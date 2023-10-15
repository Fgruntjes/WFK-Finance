terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.75.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "2.43.0"
    }
    mssql = {
      source  = "betr-io/mssql"
      version = "0.2.7"
    }
  }
  backend "azurerm" {
    container_name = "terraform"
    key            = "terraform.tfstate"
  }
}

provider "azurerm" {
  features {}

  client_id       = var.arm_client_id
  client_secret   = var.arm_client_secret
  subscription_id = var.arm_subscription_id
  tenant_id       = var.arm_tenant_id
}
provider "azuread" {
  client_id     = var.arm_client_id
  client_secret = var.arm_client_secret
  tenant_id     = var.arm_tenant_id
}
provider "mssql" {}

data "azurerm_client_config" "current" {}
data "azuread_client_config" "current" {}
data "azurerm_container_registry" "app" {
  name                = replace(var.app_project_slug, "-", "")
  resource_group_name = var.app_project_slug
}

