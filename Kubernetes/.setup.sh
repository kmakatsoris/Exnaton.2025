#!/bin/bash

# Function to install dependencies
install_dependencies() {
    echo "🚀 Installing Dependencies..."

    # Install Docker
    echo "🔹 Installing Docker..."
    sudo apt update
    sudo apt install -y docker.io
    sudo systemctl enable docker
    sudo systemctl start docker

    # Install Docker Compose
    echo "🔹 Installing Docker Compose..."
    sudo apt install -y docker-compose

    # Install Minikube
    echo "🔹 Installing Minikube..."
    curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
    sudo install minikube-linux-amd64 /usr/local/bin/minikube
    rm minikube-linux-amd64

    # Install Kubectl
    echo "🔹 Installing Kubectl..."
    curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
    chmod +x kubectl
    sudo mv kubectl /usr/local/bin/

    echo -e "\n🎉 All dependencies have been installed successfully!"
}

# Run the installation function
install_dependencies

# Pause for user confirmation before proceeding
read -p "🔹 Press any key to start Minikube and apply Kubernetes resources... " -n1 -s
echo -e "\n"

# --------------------------------------------------------
# [Main Flow]
# --------------------------------------------------------

sudo systemctl start docker
minikube stop
minikube delete
minikube start --driver=docker -p exnaton-cluster
# minikube -p exnaton-cluster ssh
# minikube -p exnaton-cluster stop
# minikube -p exnaton-cluster start
eval $(minikube -p exnaton-cluster docker-env)
docker info | grep "Name"
docker build -t exnaton-mysql:lts ../Docker/MySQL/
docker build -t exnaton-webapi:lts ../Exnaton/
# docker run -it --entrypoint bash exnaton-mysql:lts
# eval $(minikube docker-env --unset)

kubectl apply -f "./Namespaces/Namespaces.yaml" && set -e && namespaces=true || namespaces=false
kubectl apply -f "./Configs/ConfigMaps.yaml" && set -e && configmaps=true || configmaps=false
kubectl apply -f "./Configs/Secrets.yaml" && set -e && secrets=true || secrets=false
kubectl apply -f "./Deployments and Services/1db7649e-9342-4e04-97c7-f0ebb88ed1f8/Deployment_mysql.yaml" && set -e && mysqlDeployment=true || mysqlDeployment=false
kubectl apply -f "./Deployments and Services/1db7649e-9342-4e04-97c7-f0ebb88ed1f8/Deployment_seq.yaml" && set -e && seqDeployment=true || seqDeployment=false
kubectl apply -f "./Deployments and Services/1db7649e-9342-4e04-97c7-f0ebb88ed1f8/Deployment_webapi.yaml" && set -e && webapiDeployment=true || webapiDeployment=false
# kubectl get hpa -n shared-services
# kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/cloud/deploy.yaml
# kubectl apply -f IngressServices.yaml && set -e && Ingress=true || Ingress=false

clear
if [ "$namespaces" = true ]; then
  echo "Applying the namespaces. ✅"
else
  echo "Failed to apply namespaces. 😤"
fi

if [ "$configmaps" = true ]; then
  echo "Applying the configmaps. ✅"
else
  echo "Failed to apply configmaps. 😤"
fi

if [ "$secrets" = true ]; then
  echo "Applying the secrets. ✅"
else
  echo "Failed to apply secrets. 😤"
fi

if [ "$mysqlDeployment" = true ]; then
  echo "Applying the mysql Deployment. ✅"
else
  echo "Failed to apply mysql Deployment. 😤"
fi

if [ "$seqDeployment" = true ]; then
  echo "Applying the seq Deployment. ✅"
else
  echo "Failed to apply seq Deployment. 😤"
fi

if [ "$webapiDeployment" = true ]; then
  echo "Applying the webapi Deployment. ✅"
else
  echo "Failed to apply webapi Deployment. 😤"
fi

echo "🎉 Enjoy!"