# NB if you run this under a stateful CI you should export the
#    COMPOSE_PROJECT_NAME environment variable to differentiate this build
#    from the others. this is needed because docker-compose uses this name
#    as a container/network name prefix (which should be unique within the
#    CI host machine).
#    see https://docs.docker.com/compose/reference/envvars/#compose_project_name

# make sure we start from scratch.
docker-compose down
if (!(Test-Path tmp)) {
	mkdir tmp | Out-Null
}
rm tmp/*

# run the tests. then destroy everything.
try {
	docker-compose build
	docker-compose run tests
} finally {
	docker-compose down
}
