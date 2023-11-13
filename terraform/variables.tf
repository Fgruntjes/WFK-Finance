variable "app_environment" {
  type = string
}

variable "app_version" {
  type = string
}

variable "app_project_slug" {
  type = string
}

variable "arm_location" {
  type = string
}

variable "arm_tenant_id" {
  type = string
}

variable "arm_subscription_id" {
  type = string
}

variable "arm_client_id" {
  type = string
}

variable "arm_client_secret" {
  type      = string
  sensitive = true
}

variable "auth0_domain" {
  type = string
}

variable "auth0_client_id" {
  type = string
}

variable "auth0_client_secret" {
  type      = string
  sensitive = true
}

variable "nordigen_secret_id" {
  type = string
}

variable "nordigen_secret_key" {
  type      = string
  sensitive = true
}
