# 1️⃣ Build stage — compile SvelteKit
FROM node:22.12-alpine3.19 AS builder

# Set working directory
WORKDIR /app

# Copy dependency files first (for better caching)
COPY package*.json ./

# Install dependencies (clean and reproducible)
RUN npm ci

# Copy source code
COPY . .

# Build SvelteKit (SSR)
RUN npm run build


# 2️⃣ Runtime stage — minimal production image
FROM node:22.12-alpine3.19 AS runner

WORKDIR /app

# Copy only necessary runtime files
COPY --from=builder /app/build ./build
COPY --from=builder /app/package*.json ./

# Install only production dependencies
RUN npm ci --omit=dev

# Set environment variables
ENV NODE_ENV=production
ENV PORT=3000

# Expose the SSR server port
EXPOSE 3000

# Run the app
CMD ["node", "build"]
