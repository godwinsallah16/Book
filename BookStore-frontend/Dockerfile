# Frontend Dockerfile
FROM node:18-alpine AS build

WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Install OpenSSL for SSL certificate generation
RUN apk add --no-cache openssl

# Copy built assets from build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Copy SSL certificates
COPY ../certs/ /etc/nginx/certs/

# Expose ports 80 and 443
EXPOSE 80
EXPOSE 443

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
