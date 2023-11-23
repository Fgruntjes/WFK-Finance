resource "auth0_user" "test_user" {
  count           = var.app_environment != "main" ? 1 : 0
  connection_name = "Username-Password-Authentication"
  email           = "test-${var.app_environment}@test.com"
  email_verified  = true
  password        = "passpass$12$12"
}
