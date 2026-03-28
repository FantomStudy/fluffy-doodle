FROM nginx:stable-alpine AS production

COPY . /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
RUN apk add --no-cache brotli \
    && for file in /usr/share/nginx/html/Build/*.br; do brotli -d "$file" -o "${file%.br}"; done \
    && apk del brotli

EXPOSE 5174
CMD ["nginx", "-g", "daemon off;"]  
