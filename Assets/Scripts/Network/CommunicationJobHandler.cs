﻿using Unity.Burst;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using System;

public class CommunicationJobHandler : MonoBehaviour
{
    protected JobHandle m_updateHandle;

    protected Queue<IJob> jobsScheduleQueue;

    /// <summary>
    /// Queue job to the jobsScheduleQueue
    /// Allocates the queue if it is null
    /// </summary> 
    protected void QueueJob(IJob job){
        if(jobsScheduleQueue == null){
            jobsScheduleQueue = new Queue<IJob>();
        }
        jobsScheduleQueue.Enqueue(job);
    }

    /// <summary>
    /// From a set of pre-determined jobs types, the method dequeue each job from jobsScheduleQueue to schedule in the JobHandle
    /// </summary> 
    /// </summary> 
    protected void ScheduleJobsInQueue(){
        while(jobsScheduleQueue.Count>0){
            IJob job = jobsScheduleQueue.Dequeue();
            ScheduleJob(job, job.GetType());
        }
    }

    /// <summary>
    /// Schedule jobs from the types: ConnectionUpdateJob, ProcessDataJob, SendDataJob
    /// </summary> 
    void ScheduleJob(IJob job, Type type){
        if(type == typeof(ProcessDataJob)){
            m_updateHandle = ((ProcessDataJob)job).Schedule(m_updateHandle);

        }else if(type == typeof(ConnectionUpdateJob)){
            m_updateHandle = ((ConnectionUpdateJob)job).Schedule(m_updateHandle);
            
        }else if(type == typeof(SendDataJob)){
            m_updateHandle = ((SendDataJob)job).Schedule(m_updateHandle);

        }else{
            throw new Exception(string.Format("Type {0} not valid", type.ToString()));
        }
    }
}