resource "sentry_team" "default" {
  organization = var.sentry_organisation

  name = "${var.app_project_slug} ${var.app_environment}"
  slug = "${var.app_project_slug}-${var.app_environment}"
}
