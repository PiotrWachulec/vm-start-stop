# Introduction

This repository is a part of ['100 commitów'](https://100commitow.pl/) challenge. I will work on a project for 100 days in this challenge. This project aims to have a simple solution for starting and stopping virtual machines in Azure based on the schedule. The schedule will be defined in resource tags.

# Requirements

The solution needs to turn VMs on and off based on schedule within tags on the VM or RG levels. The tag on the RG level means that all VMs in the RG should be turned on/off simultaneously.
- We would avoid administrative overhead. We want to prevent secret management/rotation, manual authentication, etc.
- The solution should be monitored to see if it is working. If any problem will appear, we would like to be notified
    - Email notification
    - Slack/Discord notification
- A single solution should cover different subscriptions.

# Existing options

We have to options to achieve this goal with the existing Azure services:
- [based on Azure Functions: Start/Stop VMs v2 overview](https://learn.microsoft.com/en-us/azure/azure-functions/start-stop-vms/overview)
- [Auto-shutdown a virtual machine](https://learn.microsoft.com/en-us/azure/virtual-machines/auto-shutdown-vm?tabs=portal)

# Design 

[Miro board](https://miro.com/app/board/uXjVNmbIwqo=/?share_link_id=652932784930)