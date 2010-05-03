class Arc:
    def __init__(self, city1, city2, distance):
        self.__city1 = city1
        self.__city2 = city2
        self.__distance = distance
    
    @property
    def city1(self):
        return self.__city1
        
    @property
    def city2(self):
        return self.__city2
    
    @property
    def distance(self):
        return self.__distance
        
    def ToString(self):
        return ", ". join((self.city1, self.city2, str(self.distance)))