variable "kubeconfig_path" {
  description = "Path to the kubeconfig file, usually ~/.kube/config"
  type        = string
  default     = "~/.kube/config"
}

variable "kubeconfig_context" {
  description = "The specific context to use from your kubeconfig"
  type        = string
  default     = ""
}
