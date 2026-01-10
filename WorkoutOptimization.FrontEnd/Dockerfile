FROM node:18 AS build

# 1. HIBA JAVÍTÁSA: Beállítjuk a munkakönyvtárat, hogy ne a gyökérbe szórja a fájlokat
WORKDIR /app

COPY package*.json ./
RUN npm install

# 2. A FŐ HIBA JAVÍTÁSA: Itt hiányzott a forráskód bemásolása!
# Ez másolja be az src mappát, angular.json-t, tsconfig-ot, stb.
COPY . .

RUN npm run build -- --configuration production

FROM nginx:alpine

COPY nginx.conf /etc/nginx/conf.d/default.conf

# Ellenőrizd, hogy a dist utáni mappanév (WorkoutOptimization.FrontEnd) 
# biztosan egyezik-e azzal, ami az angular.json-ban van!
COPY --from=build /app/dist/workout-optimization.front-end/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]