terraform {
  cloud {
    # Replace "your-org" with your Terraform Cloud organization name.
    # Sign up for free at https://app.terraform.io
    organization = "your-org"

    workspaces {
      name = "beauty-studio"
    }
  }
}
