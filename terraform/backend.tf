terraform {
  cloud {
    # Replace "your-org" with your Terraform Cloud organization name.
    # Sign up for free at https://app.terraform.io
    organization = "Petrov_Gergevec"

    workspaces {
      name = "beauty-studio"
    }
  }
}
