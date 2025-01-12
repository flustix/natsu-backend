docker build -t flustix/natsu-web:latest ../natsu-frontend
docker build -t flustix/natsu:latest .

docker compose up -d