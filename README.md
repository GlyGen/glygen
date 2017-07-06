# glygen

# Containers

```
docker run --name gwu-mongo -d -p 27017:27017 -v /data/docker/gwu-mongo/configdb:/data/configdb -v /data/docker/gwu-mongo/db:/data/db mongo
```

```
docker run -d --name gwu-es -p 9200:9200 -p 9300:9300 -v /data/docker/gwu-es/data:/usr/share/elasticsearch/data elasticsearch
```

```
docker run -d --name gwu-neo4j -p 7473:7473 -p 7474:7474 -p 7687:7687 -v /data/docker/gwu-neo4j/data:/data neo4j
```
