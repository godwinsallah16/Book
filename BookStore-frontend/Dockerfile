# Frontend Dockerfile for Render
FROM node:18-alpine AS build

WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Clear npm cache and install dependencies
RUN npm cache clean --force && npm ci

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Copy built assets from build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
