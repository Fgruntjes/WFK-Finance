terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.78.0"
    }
    mssql = {
      source  = "betr-io/mssql"
      version = "0.2.7"
    }
    auth0 = {
      source  = "auth0/auth0"
      version = "1.0.0"
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
provider "mssql" {
  debug = true

}
provider "auth0" {
  domain        = var.auth0_domain
  client_id     = var.auth0_client_id
  client_secret = var.auth0_client_secret
  debug         = true
}

data "azurerm_client_config" "current" {}
data "azuread_client_config" "current" {}
data "azurerm_container_registry" "app" {
  name                = replace(var.app_project_slug, "-", "")
  resource_group_name = var.app_project_slug
}

