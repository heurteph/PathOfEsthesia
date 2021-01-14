import matplotlib as mpl
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.cbook as cbook
import matplotlib.animation as animation
import csv
import os
import sys

xdata = []
ydata = []

if len(sys.argv) < 2:
    number = 0;
else:
    number = sys.argv[1]

with open('C:/Users/Administrateur/Documents/GitHub/SoA/SoA-Unity/Assets/AnalyticsData/run' + str(number) + '.csv') as csvDataFile:
    csvReader = csv.reader(csvDataFile)
    for row in csvReader:
        xdata.append(float(row[1]))
        ydata.append(float(row[2]))

xmin = np.min(xdata)
xmax = np.max(xdata)
ymin = np.min(ydata)
ymax = np.max(ydata)

fig = plt.figure()

axes = fig.add_subplot(111)
axes.set_title("Path of Esthesia\nEvolution of the player's position on the map")    
axes.set_xlabel('X Position')
axes.set_ylabel('Y Position')
axes.set_xlim([xmin,xmax])
axes.set_ylim([ymin,ymax])

line, = axes.plot([],[])

def start():
    line.set_data([],[])
    return line,


def update(frame):
    line.set_data(xdata[:frame], ydata[:frame])
    return line,

ani = animation.FuncAnimation(fig, update, init_func=start, frames=len(xdata), blit=True, interval=20, repeat=False)

plt.show()
