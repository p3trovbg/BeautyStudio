# Read all YAML files from the k8s directory
data "kubectl_path_documents" "manifests" {
  pattern = "${path.module}/../k8s/*.yaml"
}

# Apply the manifests found
resource "kubectl_manifest" "apply_k8s" {
  for_each  = data.kubectl_path_documents.manifests.manifests
  yaml_body = each.value

  # Using pure sequential apply for simplicity, although kubectl_manifest
  # can usually infer some dependencies, defining namespace first manually
  # is often required if applying completely unknown resources concurrently.
  # But assuming the user applies them directly, kubectl_manifest handles it fairly well.
}
