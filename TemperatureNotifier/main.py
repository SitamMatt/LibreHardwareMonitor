from tkinter import Tk
from tkinter.filedialog import askopenfilename
import pandas as pd
import plotly.express as px
import sys
import os.path

if len(sys.argv) > 1 and os.path.isfile(sys.argv[1]):
    filename = sys.argv[1]
else:
    Tk().withdraw()
    filename = askopenfilename()

if filename:
    df = pd.read_csv(filename, ';', decimal=',')
    fig = px.line(x=df['DateTime'], y=df['Temperature/CPU Package'])
    fig.show()
