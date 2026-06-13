# ---- Build Frontend ----
FROM node:20-alpine AS frontend-build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ .
RUN npm run build

# ---- Build Backend ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /src
COPY backend/ZJAdmin.Api/*.csproj .
RUN dotnet restore
COPY backend/ZJAdmin.Api/ .
RUN dotnet publish -c Release -o /publish

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
RUN apk add --no-cache nginx

# nginx config
COPY docker/nginx.conf /etc/nginx/http.d/default.conf

# Copy frontend build
COPY --from=frontend-build /app/dist /usr/share/nginx/html

# Copy backend
COPY --from=backend-build /publish /app

# Startup script
COPY docker/start.sh /start.sh
RUN chmod +x /start.sh

EXPOSE 80
CMD ["/start.sh"]
