## [3.0.2](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v3.0.1...v3.0.2) (2023-02-09)


### Bug Fixes

* add custom lockName constructor to DistributedLock ([b2eabf0](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/b2eabf0a084593350689ff3d413e8e802119e7b1))
* add missing Serializable attribute to exception ([67e572f](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/67e572f0855aa9d0f80422ea89119ef8883ce096))
* add non-static RunAsync for DistributedLock ([90eaae3](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/90eaae3933f7eed83de65418b1cd572d154bfe8e))
* add nuget to dependabot ([c0d83b6](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/c0d83b64f446adfd15f12567e48da44493fb90d7))
* add option to throw/terminate on acquire lock ([37d4d1e](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/37d4d1eb07fd53afaf0ea7e5f7d441e11fa02286))
* add specific exception for acquire lock failure + use IMutex instead of DynamoDBMutex for testability ([947efbc](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/947efbc09150bc5ed9eea8672a123328713aeafc))
* cleanup ([38e8e25](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/38e8e2562141f450e16c3e95b2efb04f47430061))
* ensure CreateMutex result is not null ([396279b](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/396279bd4c9b6ab1a712f4bd6076b587d7f4545f))
* fix solution name ([9c5f645](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/9c5f645809b2ab7e40d2008fcc0559f769af70a8))
* really use VBR_SONAR_TOKEN ([b3dfc4b](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/b3dfc4b1fee0cef216ee93cc1cdc3a04a8ef1a8f))
* revert virtual method CreateMutex ([0c69d75](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/0c69d7505075dcad79544b174f49d1a3cce7f497))
* skip failing tests ([ef4ddb6](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/ef4ddb67c326125a4c7d4ee65edb2a0048595f0b))
* throw ArgumentException instead of NullReferenceException ([4b8e223](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/4b8e223b27488b7dc3bcd6ced8b7b7c323b898ed))
* use await for Async methods ([c4ac1bf](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/c4ac1bfd52e02f718a73893e26205e578010b904))
* use VBR_SONAR_TOKEN ([7c8c8dd](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/7c8c8ddd65b048696b104937b0db7dc4b7f1d750))
* version bump ([219e48a](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/219e48a5f79a6c1ea4e4c50b39a25ccaf91606a5))
* version bump ([f65e028](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/f65e028832a2f925d2913d71f98f92d8d1742d64))
* version bump ([c6a2e22](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/c6a2e222d33b5ef7742f7289a15c549d1d33da8d))

## [3.0.1](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v3.0.0...v3.0.1) (2022-09-12)


### Bug Fixes

* add query semantics & CircuitBreaker ([4c5fc9c](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/4c5fc9cfa8774bb9c24b39b4283e05f080de5c9d))
* change workflow ([a163373](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/a163373191bad928bf8c792414ff6242bf4bc2a0))
* remove unneede items in .csproj files ([f492ba3](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/f492ba3c318893a64da0fa514b335b20ff1e6de1))
* resolve PR comments ([082859d](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/082859d1e9b162b4343229909a28a28e043f1ce9))

# [3.0.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.4.0...v3.0.0) (2022-03-25)


### Features

* move to dotnet 6.0.3 ([039b7c9](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/039b7c98a89bdae63e573aa9e32f254cb49a7a40))


### BREAKING CHANGES

* move to dotnet 6.0.3

# [2.4.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.3.1...v2.4.0) (2021-06-10)


### Features

* create new constructor for DynamoDBMutex to handle no key/secret ([a1798b2](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/a1798b2ae5b2158221d3c8672be9ef970678fa94))

## [2.3.1](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.3.0...v2.3.1) (2021-05-28)


### Bug Fixes

* move to 5.0.6 ([51f275d](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/51f275d88f1687d15758a2bca68f7a153c2a0c28))

# [2.3.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.11...v2.3.0) (2021-04-29)


### Features

* add table exists check ([4c193fc](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/4c193fcd852baa17190ce627b91a7768ea42e8bd))

## [2.2.11](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.10...v2.2.11) (2021-02-02)


### Bug Fixes

* move to 5.0.2 ([59e8608](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/59e8608b2274a387f800d5fcb3dfbe90ae4c82fa))

## [2.2.10](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.9...v2.2.10) (2020-12-18)


### Bug Fixes

* correct gh actions ([a83fe50](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/a83fe501b72432e42c544ed5def73ee5bed3c4eb))
* move to 5.0.1 ([722bc76](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/722bc76e90b4c88576cb4a35d525e74ad7bcfc0e))

## [2.2.9](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.8...v2.2.9) (2020-10-29)


### Bug Fixes

* increase logging to warning when a lock is refused ([fafb0f3](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/fafb0f3eb37642046d3f8df16dcb6f0b136fcecb))

## [2.2.8](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.7...v2.2.8) (2020-09-21)


### Bug Fixes

* move to 3.1.8 ([ebe91a7](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/ebe91a7bbc379f98c8a1022a7983af02ef114b7d))

## [2.2.7](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.6...v2.2.7) (2020-07-18)


### Bug Fixes

* move to 3.1.6 ([99b6b2d](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/99b6b2d8bad8c8949e45d3c726c6cea98d268efb))

## [2.2.6](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.5...v2.2.6) (2020-06-19)


### Bug Fixes

* move to 3.1.5 ([a8e07c3](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/a8e07c3f22c59bc8a578601b9d8beabd9b0c78f7))

## [2.2.5](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.4...v2.2.5) (2020-05-20)


### Bug Fixes

* force build ([8127d45](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/8127d45a7d9ddcd325b7e3767cc91fd9d541d6ff))

## [2.2.4](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.3...v2.2.4) (2020-05-19)


### Bug Fixes

* update dynamodb deps ([cd30fb8](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/cd30fb84e8a1095395aa7a860eafe69abd0a268f))

## [2.2.3](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.2...v2.2.3) (2020-05-18)


### Bug Fixes

* move to 3.1.4 ([693d1e5](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/693d1e5156cafc57dc40896cc89f5b6d50833a78))

## [2.2.2](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.1...v2.2.2) (2020-05-13)


### Bug Fixes

* move to GH-actions ([6d2bbc1](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/6d2bbc1076774b0c37c0b888d01b22d7f410d34b))

## [2.2.1](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.2.0...v2.2.1) (2020-03-03)


### Bug Fixes

* bump netcore to 3.1.2 ([12b7c54](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/12b7c5449a23ab23d4e6036dafa194d8472cd6fc))

# [2.2.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.1.0...v2.2.0) (2020-01-31)


### Features

* upgrade netcoreapp31 and dependencies ([dd68ee2](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/dd68ee24f78e71bbc4c422bf49712eb5ba6bea99))

# [2.1.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v2.0.0...v2.1.0) (2020-01-23)


### Features

* add RunAsync ([6392b2a](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/6392b2aeb59b0d225d6dc593ac27b7b38b363317))

# [2.0.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.4.0...v2.0.0) (2020-01-10)


### Features

* add enable toggle ([#3](https://github.com/informatievlaanderen/aws-distributed-mutex/issues/3)) ([90dd72c](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/90dd72cb87dbbd5032a105967b3c49281ff046f8))


### BREAKING CHANGES

* Require a logger

# [1.4.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.3.0...v1.4.0) (2020-01-04)


### Features

* use pay per request billing ([d6aa4eb](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/d6aa4eb8db94b28a98192f5075ca25a87bdda396))

# [1.3.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.2.0...v1.3.0) (2020-01-02)


### Features

* add loading from configuration ([e2e6aff](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/e2e6aff0cc342a19239d5f572d8d00ddf1f6ff69))

# [1.2.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.1.2...v1.2.0) (2020-01-02)


### Features

* add Run to support easily getting a simple lock and releasing it ([1fe0016](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/1fe0016419a8d68caa6d381e3549ff3f93228536))

## [1.1.2](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.1.1...v1.1.2) (2019-12-19)


### Bug Fixes

* release lock ([aa07823](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/aa078235a6cd4c5f9b90896430287edd599494eb))

## [1.1.1](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.1.0...v1.1.1) (2019-12-17)


### Bug Fixes

* set the timer interval ([e7ffbce](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/e7ffbce6ba07f1120a98b292f77654e6b9fe703b))

# [1.1.0](https://github.com/informatievlaanderen/aws-distributed-mutex/compare/v1.0.0...v1.1.0) (2019-12-17)


### Features

* allow configuring table name for simple lock ([107da0f](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/107da0f3a3fb4ad05cfe4cb1d2fb9051d97704e7))

# 1.0.0 (2019-12-17)


### Features

* add simple distributed lock ([a04fedf](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/a04fedf0ea90956be3688d616a4c9f46ea2fa0b1))
* allow passing in aws key and secret ([ca58029](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/ca580297f6dd1d7cc061d1da9addc5480f38c3ea))
* prepare for release ([e41c922](https://github.com/informatievlaanderen/aws-distributed-mutex/commit/e41c9227cd5dff3b78cb761f827d27ae502e85e1))
