#!/bin/bash

# Dependencies to do the followings

sudo systemctl start docker
minikube stop
minikube delete
minikube start --driver=docker -p exnaton-cluster
minikube -p exnaton-cluster ssh
(Inside the exnaton-cluser ssh: sudo apt-get update
sudo apt-get install -y \
    libharfbuzz-dev \
    libfreetype6 \
    libfontconfig1 \
    libpng-dev \
    libgl1-mesa-glx \
    libx11-dev \
    libxrender-dev \
    libicu-dev
)
minikube -p exnaton-cluster stop
minikube -p exnaton-cluster start
eval $(minikube -p exnaton-cluster docker-env)
docker info | grep "Name"
docker build -t exnaton-mysql:lts ../Docker/MySQL/
docker build -t exnaton-webapi:lts ../Exnaton/
# docker run -it --entrypoint bash exnaton-mysql:lts
# eval $(minikube docker-env --unset)

kubectl apply -f Namespaces.yaml && set -e && namespaces=true || namespaces=false
kubectl apply -f ConfigMaps.yaml && set -e && configmaps=true || configmaps=false
kubectl apply -f Secrets.yaml && set -e && secrets=true || secrets=false
kubectl apply -f Deployment_SharedServices.yaml && set -e && Deployment_SharedServices=true || Deployment_SharedServices=false
kubectl apply -f HorizontalPodAutoscalers.yaml && set -e && HorizontalPodAutoscalers=true || HorizontalPodAutoscalers=false
# kubectl get hpa -n shared-services
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/cloud/deploy.yaml
kubectl apply -f IngressServices.yaml && set -e && Ingress=true || Ingress=false

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

if [ "$Deployment_SharedServices" = true ]; then
  echo "Applying the deployment for shared services. ✅"
else
  echo "Failed to apply deployment for shared services. 😤"
fi

if [ "$HorizontalPodAutoscalers" = true ]; then
  echo "Applying the Horizontal Pod Autoscalers. ✅"
else
  echo "Failed to apply Horizontal Pod Autoscalers. 😤"
fi

if [ "$Ingress" = true ]; then
  echo "Applying the Ingress Services. ✅"
else
  echo "Failed to apply Ingress Services. 😤"
fi
