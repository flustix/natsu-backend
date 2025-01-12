docker build -t flustix/natsu-web:latest ../natsu-frontend
docker build -t flustix/natsu:latest .

docker image push flustix/natsu-web:latest
docker image push flustix/natsu:latest