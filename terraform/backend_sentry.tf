resource "sentry_project" "backend" {
  organization = var.sentry_organisation

  name  = "${var.app_project_slug} ${var.app_environment} Backend"
  slug  = "${var.app_project_slug}-${var.app_environment}-backend"
  teams = [sentry_team.default.slug]

  platform    = "dotnet-aspnetcore"
  resolve_age = 720
}

resource "sentry_key" "backend" {
  organization = var.sentry_organisation

  project = sentry_project.backend.slug
  name    = "Key"
}
