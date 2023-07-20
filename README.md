# hackernews best story API

API plumbing to work with HackerNews API

# About

This is a REST API written in C# 7.0. The API runs in a docker container on Linux or Windows.

# Dependencies

The API uses Redis cache to cache results from HackerNews. The best story id list is cached until midnight and stories are kept for 7 days before being cleared. This should reduce the hits on HackerNews.

# Prerequisites

1. Docker - https://docs.docker.com/engine/install/
2. Postman - https://www.postman.com/downloads/

# Installation

1. navigate to the /src/apps/api folder
2. open a command window
3. run `docker-compose up --build` (this will build the image, run unit tests and launch the container). It takes a couple of minutes depending on your OS/machine.
4. wait for the `src-api-1 | [HH:MM:SS INF] Starting up API..` log on the console output then on your browser navigate to `http://localhost:5134` to see the swagger doc.
5. open Postman and load the collection in the /src/apps folder to see examples of the calls.

# Debugging

1. In a command window run `docker run --name redis -d -p 6379:6379 redis`
2. Navigate to /src/apps and open the api.sln solution in Visual Studio 2022
3. Load the **http** profile and run. This should launch a browser window to the swagger doc

# Troubleshooting

1. If the default port is already in use , you can change it to a free port in the docker-compose file described above.
2. If the container keeps exiting it could be because redis is already running your machine. In that case, you can change the host port number in the docker compose and suffix the REDIS_SERVER environment variable with the new port (e.g. localhost:12344 ) and re-run.
3. Some organisations do not allow direct download of docker images from dockerhub. You may need to tweak the redis image name to point to a reachable docker repo mirror.

# Environment variables

- MAX_STORY_COUNT (default 100) - the maximum number of stories that can be returned in one call

- MAX_CONCURRENT_REQUESTS (default 5) - the number of concurrent requests allowed to hackernews. This is a per instance (container) setting - see notes about rate limiting in the future enhancements section

- REDIS_SERVER (default localhost) - the server location of the redis server
- MAX_REDIS_PING_TIME_SECONDS (default 30) - number of seconds to wait for a redis ping to return before failing a healthcheck
- HACKER_NEWS_URI - base hackernews uri
- TIMEOUT_SECONDS (default 60) - max wait for hackernews to return
- RETRIES - default(5) - number of retries after transient faults making hackernews calls
- RETRY_BACKOFF - default(2) - number of exponential seconds between retries to hackernews
- MAX_STORY_AGE_DAYS - default(7) - number of days to cache a story

# Methods

1. /health/hello
   This call is a GET call and expects no arguments. Calling it checks to see if HackerNews is reachable and the cache is up. If either check fail it returns a http 500 error otherwise it returns http 200. It can be used to automatically restart the API container if either dependency were to degrade. See the healthcheck on the docker-compose.

2. /news/beststories
   This call expects a body definining two optional arguments.

- maxCount : this is the maximum number of stories to be returned
- skipCount: this is the number of stories to be skipped
  Combining the two allows you to "page" the results. For example to get the best 40 stories, you would set the first call to maxCount=20 and skipCount=0, this would return 20 stories. You would make a second call and set maxCount=20 and skipCount=20, this would return the next 20 stories and so on. MaxCount cannot be set to more than 100.
  Each result is sorted by the story score with the highest scoring first. Note that the sorting is only done within the resultset.

# Rate Limiting

To prevent abuse of the HackerNews API, we implemented request throttling within the API.

# Assumptions

# Future enhancements

- Rate limiting at the API gateway level. This is prefereable to request throttling done at class level as it can be done across multiple instances of the API.

- Asynchronous handling. During busy periods the hackernews API may be slow to respond which could timeout the clients of our API. A good way to mitigate this would be to send requests to a queue and return an id. The client would then use the id to retrieve the results at a later time. We would need to implement a microservice to process these requests.

- Gather metrics to monitor performance insights to guide on how to scale the API based on usage patterns.

- Centralise logging using aggregation tools such as GLP stack to aid in supporting the API

- Add a pre-deployment smoke test to ensure you only deploy good releases.
