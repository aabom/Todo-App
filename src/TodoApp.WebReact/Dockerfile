# Build from this directory:  docker build src/TodoApp.WebReact

FROM node:24-alpine AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
# Leave VITE_API_URL unset to call /graphql on the same origin, which nginx proxies below.
# Set it only to bake in an API on another origin.
ARG VITE_API_URL
RUN npm run build

# nginx-unprivileged runs as a non-root user and listens on 8080.
FROM nginxinc/nginx-unprivileged:1.28-alpine AS runtime
# The entrypoint fills in ${API_UPSTREAM} when the container starts, so one image works
# against any API address.
COPY nginx/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html
ENV API_UPSTREAM=http://api:8080
EXPOSE 8080
