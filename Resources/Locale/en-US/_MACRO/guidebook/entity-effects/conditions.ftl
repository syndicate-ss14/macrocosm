entity-condition-guidebook-status-effect-duration-min =
    the target has at least {NATURALFIXED($min, 3)} {MANY("second", $min)} of {$effect}
entity-condition-guidebook-status-effect-duration-max =
    the target has at most {NATURALFIXED($max, 3)} {MANY("second", $max)} of {$effect}
entity-condition-guidebook-status-effect-duration-minmax =
    the target has between {NATURALFIXED($min, 3)} and {NATURALFIXED($max, 3)} {MANY("second", $max)} of {$effect}

entity-condition-guidebook-status-effect-duration =
    { $max ->
        [2147483648] {entity-condition-guidebook-status-effect-duration-min}
        *[other]
        { $min ->
            [0] {entity-condition-guidebook-status-effect-duration-max}
            *[other] {entity-condition-guidebook-status-effect-duration-minmax}
        }
    }
