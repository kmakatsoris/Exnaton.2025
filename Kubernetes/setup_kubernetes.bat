@echo off
setlocal enabledelayedexpansion

:: --------------------------------------------------------
:: Install Dependencies
:: --------------------------------------------------------
echo 🚀 Installing Dependencies...

:: Install Chocolatey (if not installed)
where choco >nul 2>nul
if %errorlevel% neq 0 (
    echo 🔹 Installing Chocolatey...
    powershell -NoProfile -ExecutionPolicy Bypass -Command ^
        "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
    exit /b
) else (
    echo ✅ Chocolatey is already installed.
)

:: Install Docker
echo 🔹 Installing Docker...
choco install -y docker-desktop
if %errorlevel% neq 0 echo ❌ Docker installation failed!

:: Install Docker Compose
echo 🔹 Installing Docker Compose...
choco install -y docker-compose
if %errorlevel% neq 0 echo ❌ Docker Compose installation failed!

:: Install Minikube
echo 🔹 Installing Minikube...
choco install -y minikube
if %errorlevel% neq 0 echo ❌ Minikube installation failed!

:: Install Kubectl
echo 🔹 Installing Kubectl...
choco install -y kubernetes-cli
if %errorlevel% neq 0 echo ❌ Kubectl installation failed!

echo 🎉 All dependencies installed successfully!
pause

:: --------------------------------------------------------
:: Start Minikube and Docker
:: --------------------------------------------------------
echo 🚀 Starting Minikube and Docker...

:: Start Docker (if not already running)
net start com.docker.service

:: Restart Minikube cluster
minikube stop
minikube delete
minikube start --driver=docker -p exnaton-cluster
minikube -p exnaton-cluster docker-env | cmd

:: Build Docker Images
docker build -t exnaton-mysql:lts ../Docker/MySQL/
docker build -t exnaton-webapi:lts ../Exnaton/

:: Apply Kubernetes Configurations
kubectl apply -f "./Namespaces/Namespaces.yaml"
kubectl apply -f "./Configs/ConfigMaps.yaml"
kubectl apply -f "./Configs/Secrets.yaml"
kubectl apply -f "./Deployments and Services/1db7649e-9342-4e04-97c7-f0ebb88ed1f8/Deployment_mysql.yaml"
kubectl apply -f "./Deployments and Services/1db7649e-9342-4e04-97c7-f0ebb88ed1f8/Deployment_seq.yaml"
kubectl apply -f "./Deployments and Services/1db7649e-9342-4e04-97c7-f0ebb88ed1f8/Deployment_webapi.yaml"

:: Output Status Messages
echo 🎉 Kubernetes resources applied successfully!
pause
