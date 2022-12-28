# Scripts

The influxdb container contains an entrypoint located in the relative root (`./entrypoint.sh`)  
This file contains the following snippet 

```sh
# Allow users to mount arbitrary startup scripts into the container,
# for execution after initial setup/upgrade.
declare -r USER_SCRIPT_DIR=/docker-entrypoint-initdb.d

# Check if user-defined setup scripts have been mounted into the container.
function user_scripts_present () {
    if [ ! -d ${USER_SCRIPT_DIR} ]; then
        return 1
    fi
    test -n "$(find ${USER_SCRIPT_DIR} -name "*.sh" -type f -executable)"
}

# Execute all shell files mounted into the expected path for user-defined startup scripts.
function run_user_scripts () {
    if [ -d ${USER_SCRIPT_DIR} ]; then
        log info "Executing user-provided scripts" script_dir ${USER_SCRIPT_DIR}
        run-parts --regex ".*sh$" --report --exit-on-error ${USER_SCRIPT_DIR}
    fi
}
```

The scripts folder we are currently in is volume-mapped to the `/docker-entrypoint-initdb.d` directory inside the container.  
This means that all `.sh` files are executed after the influxdb setup was completed.
