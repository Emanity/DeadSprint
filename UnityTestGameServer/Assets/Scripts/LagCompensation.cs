using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This will handle to get the input of the user at real time
 * by removing all factors of network lag
 */
public class LagCompensation
{
    /* a data structure which stores player and its position for 1 second (example: 30 positions
     * will be stored if the tickrate is 30)
     * variable for interpolation delay which will be stored in constants, in the future it will be moved
     * because I plan on letting the user choose
     * Note: we should choose a limit for client ping, and if they reach the limit their connection
     * will then be rejected.
     */

    public void estimate()
    {
        /* get client's latency from a packet, then the latency will be transformed (or its already) to ms(milliseconds)
         * then get the number ticks per ms of their latency (example: 90ms will be 3ticks if tickrate is 30)
         * NOTE: we must take note that the client will have interpolation, so either we have a constant interpolation rate on the client
         * or send the interpolation rate with the latency packet, if clients are able to modify interpolation rate.
         * We will be taking in the client's interpolated position which means more latency, so overall
         * it will look like this lagCompensation = (currentTick - latencyTick) - (interpolationTick/delayTick)
         */
    }

    //create other methods to seperate calculations, to make it readable and modular
}
