FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./

RUN npm install
COPY . .

RUN npm run build

FROM caddy:alpine
COPY --from=build /app/dist/What-Two-Play/browser /var/www/html

#COPY --from=build /app/dist/What-Two-Play/browser/app-config.json /usr/share/caddy/app-config.json

COPY Caddyfile /etc/caddy/Caddyfile
EXPOSE 80
ENTRYPOINT ["caddy", "run", "--config", "/etc/caddy/Caddyfile"]
