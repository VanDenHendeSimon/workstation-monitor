# Queries

## Get application-samples
```flux
from(bucket: "application")
  |> range(start: v.timeRangeStart, stop: v.timeRangeStop)
  |> filter(fn: (r) => r["_field"] == "v")
  |> aggregateWindow(every: 100ms, fn: last, createEmpty: false)
  |> yield(name: "last")
```
