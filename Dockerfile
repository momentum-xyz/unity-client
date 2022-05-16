FROM nginx:alpine as production-build

ADD ./docker_assets/nginx.conf /etc/nginx/nginx.conf

## Remove default nginx index page
RUN rm -rf /usr/share/nginx/html/*

ADD build/WebGL/WebGL/Build /usr/share/nginx/html
# COPY pthread /usr/share/nginx/html/pthread

# RUN sed -i 's/Mac OS X (10\[\\\.\\_\\d\]+)/Mac OS X (1.\[\\\.\\_\\d\]\+)/' /usr/share/nginx/html/UnityLoader.js

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
