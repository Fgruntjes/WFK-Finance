terraform {
  required_version = ">= 1.6.3"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.84.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 2.46.0"
    }
    mssql = {
      source  = "betr-io/mssql"
      version = "~> 0.2.7"
    }
    auth0 = {
      source  = "auth0/auth0"
      version = "~> 1.0.0"
    }
    local = {
      source  = "hashicorp/local"
      version = "~> 2.4.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.5.1"
    }
    time = {
      source  = "hashicorp/time"
      version = "~> 0.9.1"
    }
    sentry = {
      source  = "jianyuan/sentry"
      version = "~> 0.11.2"
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
provider "auth0" {
  domain        = var.auth0_domain
  client_id     = var.auth0_client_id
  client_secret = var.auth0_client_secret
}
provider "sentry" {
  token = var.sentry_token
}

data "azurerm_container_registry" "app" {
  name                = replace(var.app_project_slug, "-", "")
  resource_group_name = var.app_project_slug
}

data "azuread_client_config" "current" {}
